export function getTime(date: Date): string {
    let hours = date.getHours().toString();
    let minutes = date.getMinutes().toString();
    
    hours = hours.length === 1 ? "0" + hours : hours;
    minutes = minutes.length === 1 ? "0" + minutes : minutes;

    return hours + ":" + minutes; 
}

export function getNowUtc(): Date {
    const now = new Date();
    const utc = new Date(now.getTime() + now.getTimezoneOffset() * 60000);
    return utc;
}

export function getTimezoneOffsetMinutes(utcDate: Date, localDate: Date): number {
    return (localDate.getTime() - utcDate.getTime()) / 60000;
}

export function getNowLocalTime(timezoneOffsetMinutes: number): Date {
    const now = new Date();
    const utcMs = now.getTime() + now.getTimezoneOffset() * 60000;
    const local = new Date(utcMs + timezoneOffsetMinutes * 60000);
    return local;
}

export function getDateTime(date: Date): string {
    return getTime(date) + " " + getDate(date);

}

export function convertUtcToLocal(utcDate: Date): Date {
    const timezoneOffsetMinutes = new Date().getTimezoneOffset();
    const localMs = utcDate.getTime() - timezoneOffsetMinutes * 60000;
    return new Date(localMs);
}

export function getFullDateTime(date: Date): string {
    return getTime(date) + " " + getDate(date) + "/" + date.getFullYear();

}

export function getTimeWithDay(data: Date): string {
    return getTime(data) + ", " + getDay(data);
}

function getDay(date: Date): string {
    switch (date.getDay()) {
        case 0:
            return "Sunday";
        case 1:
            return "Monday";
        case 2:
            return "Tuesday";
        case 3:
            return "Wednesday";
        case 4:
            return "Thursday";
        case 5:
            return "Friday";
        case 6:
            return "Saturday";
        default:
            return "";
    }
}

export function getDate(date: Date): string {
    let day = date.getDate().toString();
    let month = (date.getMonth() + 1).toString();


    day = day.length === 1 ? "0" + day : day;
    month = month.length === 1 ? "0" + month : month;
    return day + "/" + month;
}