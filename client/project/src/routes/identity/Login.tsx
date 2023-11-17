import { MouseEvent, useContext, useState } from "react";
import { ILoginData } from "../../dto/ILoginData";
import { IdentityService } from "../../services/IdentityService";
import { JwtContext } from "../Root";
import LoginFormView from "./LoginFormView";
import { useNavigate } from "react-router-dom";


const identityService = new IdentityService();
const Login = () => {
    const navigate = useNavigate();

    const [values, setInput] = useState({
        email: "",
        password: "",
    } as ILoginData);

    const [validationErrors, setValidationErrors] = useState([] as string[]);

    const handleChange = (target: EventTarget & HTMLInputElement) => {

        setInput({ ...values, [target.name]: target.value });
    }

    const {setJwtResponse} = useContext(JwtContext);


    const onSubmit = async (event: MouseEvent) => {
        event.preventDefault();

        if (values.email.length === 0 || values.password.length === 0) {
            setValidationErrors(["Bad input values!"]);
            return;
        }
        // remove errors
        setValidationErrors([]);

        var jwtData = await identityService.login(values);

        if (jwtData === undefined) {
            // TODO: get error info
            setValidationErrors(["wrong email or password"]);
            return;
        } 

        if (jwtData === null) {
            setValidationErrors(["You have not been verified yet! Try again later!"]);
            return;
        } 

        if (setJwtResponse){
             setJwtResponse(jwtData);
             navigate("/");
        }
    }

    return (
        <LoginFormView values={values} handleChange={handleChange} onSubmit={onSubmit} validationErrors={validationErrors} />
    );
}

export default Login;