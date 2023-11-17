const Error = (props: {errorMessage: string}) => {

    return (
        <div className="text-center" id="error-page" style={{"fontSize" : "2rem"}}>
            <h1 style={{"fontSize" : "4rem", "paddingBottom" : "2rem"}}>Oops!</h1>
            <p>This page is not available.</p>
            <p>
                <i>{props.errorMessage}</i>
            </p>
        </div>
    );
}

export default Error;
