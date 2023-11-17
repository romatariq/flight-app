import { MouseEvent, useState } from "react";
import { useNavigate } from "react-router-dom";
import { IRegisterData } from "../../dto/IRegisterData";
import { IdentityService } from "../../services/IdentityService";
import RegisterFormView from "./RegisterFormView";


const identityService = new IdentityService();
const Register = () => {

    const navigate = useNavigate();

    const [values, setInput] = useState({
        password: "",
        confirmPassword: "",
        email: "",
        firstName: "",
        lastName: "",
    } as IRegisterData);

    const [validationErrors, setValidationErrors] = useState([] as string[]);

    const handleChange = (target: EventTarget & HTMLInputElement) => {
        setInput({ ...values, [target.name]: target.value });
    }

    const onSubmit = async (event: MouseEvent) => {
        event.preventDefault();


        // TODO - give correct errors
        if (values.firstName.length === 0 || values.lastName.length === 0 || values.email.length === 0 || values.password.length === 0 || values.confirmPassword.length === 0) {
            setValidationErrors(["Fields cannot be empty!"]);
            return;
        }
        if (values.password !== values.confirmPassword) {
            setValidationErrors(["Confirm password has to match password!"]);
            return;
        }

    
        // remove errors
        setValidationErrors([]);
        
        var jwtData = await identityService.register(values);

        if (jwtData === undefined) {
            // TODO: get error info
            setValidationErrors(["no jwt - invalid inputs"]);
            return;
        }
        navigate("/login");
    }

    return (
        <RegisterFormView values={values} handleChange={handleChange} onSubmit={onSubmit} validationErrors={validationErrors} />
    );
}

export default Register;
