import { Link, useParams } from "react-router-dom";
import { UserNotificationService } from "../services/UserNotificationService";
import { JwtContext } from "./Root";
import { FormEventHandler, useContext, useEffect, useState } from "react";
import { IUserFlightWithNotifications } from "../domain/IUserFlightWithNotifications";
import { getAirportTitleMinified } from "../helpers/AirportHelpers";
import { TNavigationItem } from "../dto/TNavigationItem";
import NavigationLink from "../components/NavigationLink";
import { IMinimalUserFlightNotification } from "../dto/IMinimalUserFlightNotification";
import { IUserNotification } from "../domain/IUserNotification";
import ValidationErrors from "../components/ValidationErrors";

const userNotificationService = new UserNotificationService();

const UserFlightNotifications = () => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const {flightId} = useParams<{flightId: string}>();
    const [userFlightWithNotifications, setUserFlightWithNotifications] = useState(null as IUserFlightWithNotifications | null);
    const [userNotifications, setUserNotifications] = useState([] as IUserNotification[]);
    const [validationErrors, setValidationErrors] = useState([] as string[]);

    userNotificationService.initJwtResponse(jwtResponse, setJwtResponse);


    const handleAdd = async (e: React.FormEvent) => {
        e.preventDefault();
        setValidationErrors([]);
            
        const form = e.target as HTMLFormElement;
        const minutesElement = form["minutes"] as HTMLInputElement;
        const beforeAfterElement = form["beforeAfter"] as HTMLSelectElement;
        const notificationElement = form["notification"] as HTMLSelectElement;
        
        const minutes = Number(minutesElement.value);
        const beforeAfter = beforeAfterElement.value;
        const notificationId = notificationElement.value;

        if (!userFlightWithNotifications || Number.isNaN(minutes) || !beforeAfter || !notificationId) {
            setValidationErrors(["Invalid data"]);
            return
        };

        if (minutes < 0 || minutes > 600) {
            setValidationErrors(["Minutes must be between 0 and 600"]);
            return;
        }
        const data: IMinimalUserFlightNotification = {
            minutesFromEvent: minutes * (beforeAfter === "before" ? -1 : 1),
            notificationId: notificationId,
            userFlightId: userFlightWithNotifications.id!
        }

        if (userNotifications.some(un => un.minutesFromEvent === data.minutesFromEvent && un.notificationId === data.notificationId)) {
            setValidationErrors(["Notification already exists"]);
            return;
        }

        const res = await userNotificationService.addNotification(data);
        if (res) {
            setUserNotifications([...userNotifications, res]);
        }
    }

    const handleDelete = async (id: string) => {
        const res = await userNotificationService.deleteNotification(id);
        if (res) {
            setUserNotifications(userNotifications.filter(un => un.id !== id));
        }
    }

    useEffect(() => {
        let isCancelled = false;
        const fetchData = async () => {
            if (!flightId) return;

            const res = await userNotificationService.getUserFlightWithNotifications(flightId);
            if (res && !isCancelled) {
                setUserFlightWithNotifications(res);
                setUserNotifications(res.userNotifications);
            }
        };
        fetchData();
        return () => {
            isCancelled = true;
        }
    }, [flightId]);

    if (!userFlightWithNotifications) return <></>;


    return (
        <div>
            <MyNavigationLinks flightIata={userFlightWithNotifications.flightIata} flightId={flightId!} />
            <AirportsHeader data={userFlightWithNotifications} />
            <h2 className="text-center">Notification settings</h2>

            <div className="user-notification-box">
                <AddNotificationForm data={userFlightWithNotifications} handleSubmit={handleAdd} validationErrors={validationErrors}/>
                
                {userNotifications.map(notification => {
                    return <UserNotification key={notification.id} data={notification} handleDelete={handleDelete} />
                })}
            </div>
        </div>
    );
}

const AddNotificationForm = (props: {data: IUserFlightWithNotifications, handleSubmit: FormEventHandler<HTMLFormElement>, validationErrors: string[]}) => {
    return (
        <>
        <ValidationErrors validationErrors={props.validationErrors} />
        
        <div className="user-notification-form">
        <form onSubmit={props.handleSubmit}>
            <input  placeholder="0" name="minutes" /> minutes
            <select name="beforeAfter">
                <option value="before">before</option>
                <option value="after">after</option>
            </select>

            <select name="notification">
            {props.data.allNotificationTypes.map(notification => {
                return (
                    <option key={notification.id} value={notification.id}>{notification.type.toLowerCase()}</option>
                    );
                })}
            </select>
            <button type="submit" className="add">+</button>
        </form>
        </div>
        </>
    );
};

const UserNotification = (props: {data: IUserNotification, handleDelete: (id: string) => Promise<void> }) => {
    const mins = Math.abs(props.data.minutesFromEvent);
    return (
        <div className="user-notification-card">
            <div>
                {mins} {"minute" + (mins === 1 ? "" : "s")}
                {props.data.minutesFromEvent < 0 ? " before " : " after "}
                {props.data.notificationType.toLowerCase() + " "}
            </div>
            <button className="delete" onClick={() => props.handleDelete(props.data.id!)}>-</button>
        </div>
    );
};


const MyNavigationLinks = (props: {flightIata: string, flightId: string}) => {
    const items: TNavigationItem[] = [
        ["/flights/", "my-flights"],
        [`/flights/flight/${props.flightId}`, props.flightIata]
    ];
    return NavigationLink({items: items, lastItem: "edit"});
}

const AirportsHeader = (props: {data: IUserFlightWithNotifications}) => {
    return (
        <h1 className="text-center">
            <Link to={`/airports/${props.data.departureAirportIata}`} className="black-link silent-link">
                {getAirportTitleMinified(props.data.departureAirportName, props.data.departureAirportIata)}
            </Link>
            {" -> "}
            <Link to={`/airports/${props.data.arrivalAirportIata}`} className="black-link silent-link">
                {getAirportTitleMinified(props.data.arrivalAirportName, props.data.arrivalAirportIata)}
            </Link>
        </h1>
    );
}

export default UserFlightNotifications;