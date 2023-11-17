const ValidationErrors = (props: {validationErrors: string[]}) => {
    if (props.validationErrors.length === 0) {
        return <></>;
    }
    else {
        return (
            <ul className="text-danger">
              {
                props.validationErrors.map((error) =>
                  <li key={error}>
                    {error}
                  </li>
              )}
            </ul>
        );
    }
}

export default ValidationErrors;