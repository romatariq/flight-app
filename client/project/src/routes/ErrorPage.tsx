import { Link, useRouteError } from "react-router-dom";

interface IError {
    statusText?: string,
    message?: string,
}

const ErrorPage = () => {
    const error = useRouteError() as IError;

    return (
        <div id="error-page" className="text-center">
            <h1>Oops!</h1>
            <p>Sorry, an unexpected error has occurred.</p>
            <p>
                <i>{error.statusText || error.message}</i>
            </p>
            <Link to={"/"}>Home</Link>
        </div>
    );
}

export default ErrorPage;
