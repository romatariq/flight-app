using System.Net;
using System.Net.Mail;
using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using App.Domain.Identity;
using App.Mappers.AutoMappers.BLL;
using Base.BLL;
using Base.Contracts;
using Microsoft.EntityFrameworkCore;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class UserFlightNotificationService :
    BaseEntityService<Dal.UserFlightNotificationInfo, Bll.UserFlightNotificationInfo,
        IUserFlightNotificationRepository>, IUserFlightNotificationService
{
    protected IAppUOW Uow;
    protected readonly IMapper<Dal.UserFlightWithNotifications, Bll.UserFlightWithNotifications> DetailsMapper;


    public UserFlightNotificationService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.UserFlightNotificationRepository, new AutoMappers.BLL.UserFlightNotificationMapper(mapper))
    {
        Uow = uow;
        DetailsMapper = new UserFlightWithNotificationsMapper(mapper);
    }


    public async Task<bool> DeleteDtoAsync(Guid id, AppUser appUser)
    {
        return await Uow.UserFlightNotificationRepository.DeleteDtoAsync(id, appUser);
    }

    public async Task<Bll.UserFlightNotificationInfo> AddDtoAsync(Guid userFlightId, int minutesFromEvent, Guid notificationTypeId, AppUser appUser)
    {
        return Mapper.Map(
            await Uow.UserFlightNotificationRepository.AddDtoAsync(userFlightId, minutesFromEvent, notificationTypeId, appUser)
        )!;
    }

    public async Task<Bll.UserFlightWithNotifications?> GetUserFlightWithNotifications(Guid flightId, AppUser appUser)
    {
        return DetailsMapper.Map(
            await Uow.UserFlightNotificationRepository.GetUserFlightWithNotifications(flightId, appUser)
        );
    }

    public async Task SendNotifications()
    {
        var adminEmail = Environment.GetEnvironmentVariable("DOTENV_EMAIL_USERNAME");
        var adminPassword = Environment.GetEnvironmentVariable("DOTENV_EMAIL_PASSWORD");

        if (adminEmail == null || adminPassword == null)
        {
            throw new Exception(".env file is missing email credentials");
        }

        await Task.Delay(new Random().Next(0, 2000));
        var notifications = await Uow
            .UserFlightNotificationRepository.GetAllCurrentNotificationsAsync();

        if (!notifications.Any()) return;
        
        // delete instantly to avoid duplicates
        Uow.UserFlightNotificationRepository.DeleteAsync(
            notifications.Select(x => x.Id));
        var changes = await Uow.SaveChangesAsync();
        if (changes == 0) return;


        using var smtpClient = new SmtpClient
        {
            Host = "smtp.gmail.com",
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(adminEmail, adminPassword)
        };

        using var mailMessage = new MailMessage
        {
            From = new MailAddress(adminEmail),
            Subject = "Flight notification",
            IsBodyHtml = true,
        };

        foreach (var notification in notifications)
        {
            // TODO: admin email should be an actual email
            if (notification.UserEmail.Contains("@app.com"))
            {
                notification.UserEmail = adminEmail;
            }

            var time = Math.Abs(notification.MinutesFromEvent);
            var plural = time == 1 ? "" : "s";

            mailMessage.To.Clear();
            mailMessage.Subject = $"Flight {notification.FlightIata}";
            mailMessage.Body = $"""
                 <h1>Hello, {notification.UserFirstName}!</h1>
                 <h3>New notification about your flight from {notification.DepartureAirportName} to {notification.ArrivalAirportName}.</h3>
                 <h4>
                    Your flight's {notification.NotificationType.ToLower()} 
                    {(notification.MinutesFromEvent > 0 ? "is in" : "was")} 
                    {time} minute{plural} 
                    {(notification.MinutesFromEvent > 0 ? "" : "ago")}.
                 </h4>
             """;
            mailMessage.To.Add(notification.UserEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}