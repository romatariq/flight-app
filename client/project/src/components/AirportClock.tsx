import { useEffect, useState } from "react";
import { getNowLocalTime, getTime } from "../helpers/TimeHelpers";

export const AirportClockBasic = (props: {timeOffSetInMinutes: number | undefined}) => {

    const [date, setDate] = useState(undefined as Date | undefined);

    useEffect(() => {
        if (props.timeOffSetInMinutes === undefined) return;
        setDate(getNowLocalTime(props.timeOffSetInMinutes));
    }, [props.timeOffSetInMinutes]);

    useEffect(() => {
        if (!date) return;
        const ms = (60 - date.getSeconds()) * 1000;
        const interval = setInterval(() => {
            setDate(new Date(date.getTime() + ms));
        }, ms);
        return () => clearInterval(interval);
    }, [date]);



    return (
        !date ?  
        <></> : 
        <span>{getTime(date)}</span>
    )
}

const AirportClock = (props: {timeOffSetInMinutes: number | undefined}) => {

    if (!props.timeOffSetInMinutes) return <></>;

    return (
        <h1 className="time text-right">
            <AirportClockBasic timeOffSetInMinutes={props.timeOffSetInMinutes}/>
        </h1>
    )
};

export default AirportClock;