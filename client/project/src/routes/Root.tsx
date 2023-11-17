import { createContext, useEffect, useState } from "react";
import { Outlet } from "react-router-dom";
import Header from "../components/Header";
import { IJWTResponse } from "../dto/IJWTResponse";
import { isUserAdmin } from "../helpers/IdentityHelpers";
import { NotificationService } from "../services/admin/NotificationService";

export const JwtContext = createContext<{
    jwtResponse: IJWTResponse | null,
    setJwtResponse: ((data: IJWTResponse | null) => void) | null
}>({ jwtResponse: null, setJwtResponse: null });


const notificationService = new NotificationService();
const Root = () => {
    const [jwtResponse, setJwtResponse] = useState(null as IJWTResponse | null);
    notificationService.initJwtResponse(jwtResponse, setJwtResponse);

    useEffect(() => {
        const notificationTrigger = async () => {
            if (!jwtResponse || !isUserAdmin(jwtResponse)) return;
            await notificationService.sendTrigger();
        };

        const interval = setInterval(async () => {
            await notificationTrigger();
        }, 1000 * 60);
        notificationTrigger();
        return () => clearInterval(interval);
      }, [jwtResponse]);

    return (
        <JwtContext.Provider value={{ jwtResponse, setJwtResponse }}>
            <Header />

            <div className="container">
                <main role="main" className="pb-3">
                    <Outlet />
                </main>
            </div>

        </JwtContext.Provider>
    );
}

export default Root;
