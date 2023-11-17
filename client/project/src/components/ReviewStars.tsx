const ReviewStars = (props: { rating: number, setRating: undefined | React.Dispatch<React.SetStateAction<number>> }) => {
    
    const handleClick = (i: number): void => {
        if (props.setRating) {
            props.setRating(i);
        }
    }
    
    return (
        <div>            {
            Array.from(Array(5).keys()).map(i => {
                i = i + 1;
                return ( 
                    <span key={i} 
                        className={"fa fa-star" + (props.rating >= i ? " checked" : "") + (props.setRating ? " user-rating" : "")}
                        onClick={() => {handleClick(i)}} >
                    </span>
                )
            })}
        </div>
    );
}

export default ReviewStars;