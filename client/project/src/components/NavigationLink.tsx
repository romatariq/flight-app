import { Link } from "react-router-dom";
import { TNavigationItem } from "../dto/TNavigationItem";
import React from "react";

const NavigationLink = (props: {items: TNavigationItem[], lastItem: string}) => {
    const separator = "/ ";
    return (
        <div className="back-link">
            {props.items.map(item => {
                return (
                    <React.Fragment key={item[0]}>
                        <Link key={item[0]} to={item[0]}>{item[1]}</Link> {separator}
                    </React.Fragment>
                );
            })}
            {props.lastItem}
        </div>
    ); 
}

export default NavigationLink;