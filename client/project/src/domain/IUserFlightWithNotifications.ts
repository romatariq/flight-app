import { IBaseEntity } from "./IBaseEntity";
import { INotification } from "./INotification";
import { IUserNotification } from "./IUserNotification";

export interface IUserFlightWithNotifications extends IBaseEntity {
    departureAirportName: string;
    arrivalAirportName: string;
    departureAirportIata: string;
    arrivalAirportIata: string;
    flightIata: string;
    userNotifications: IUserNotification[];
    allNotificationTypes: INotification[];
}