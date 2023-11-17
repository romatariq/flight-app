import { IBaseEntity } from "./IBaseEntity";

export interface IUserNotification extends IBaseEntity {
    
    notificationId: string;
    notificationType: string;
    minutesFromEvent: number;
}