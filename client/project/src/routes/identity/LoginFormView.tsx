import { MouseEvent } from "react";
import ValidationErrors from "../../components/ValidationErrors";
import { ILoginData } from "../../dto/ILoginData";
import '../../styles/login.css';

interface IProps {
    values: ILoginData;

    validationErrors: string[];

    handleChange: (target: EventTarget & HTMLInputElement) => void;

    onSubmit: (event: MouseEvent) => void;

}

const LoginFormView = (props: IProps) => {
    return (
        <form className="form-signin w-100 m-auto">
            <h2>Login</h2>
            <hr />

            <ValidationErrors validationErrors={props.validationErrors} />

            <div className="form-floating mb-3">
                <input
                    onChange={(e) => props.handleChange(e.target)}
                    value={props.values.email}
                    className="form-control" autoComplete="username" aria-required="true" placeholder="name@example.com" type="email"
                    id="Input_Email" name="email" />
                <label htmlFor="Input_Email">Email</label>
            </div>
            <div className="form-floating mb-3">
                <input
                    onChange={(e) => props.handleChange(e.target)}
                    value={props.values.password}
                    className="form-control" autoComplete="new-password" aria-required="true" placeholder="password" type="password"
                    id="Input_Password" maxLength={100} name="password" />
                <label htmlFor="Input_Password">Password</label>
            </div>

            <button
                onClick={(e) => props.onSubmit(e)}
                id="registerSubmit" className="w-100 btn btn-lg btn-primary">Login</button>

        </form>
    );
}

export default LoginFormView;