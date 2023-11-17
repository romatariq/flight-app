import { useEffect, useRef } from "react";
import { IReviewCategory } from "../domain/IReviewCategory";
import { ReviewService } from "../services/ReviewService";


const reviewService = new ReviewService();

const ReviewCategories = (props: {
    selectedCategoryId: string,
    setDefaultCategory: boolean,
    setSelectedCategoryId: React.Dispatch<React.SetStateAction<string>>,
    setPage: React.Dispatch<React.SetStateAction<number>> | undefined
}) => {
    
    const categories = useRef(undefined as IReviewCategory[] | undefined);
    const setSelectedCategoryId = useRef(props.setSelectedCategoryId);

    useEffect(() => {
        let isCancelled = false;
        const fetchCategories = async () => {
            const fetchedCategories = await reviewService.getCategories();
            if (fetchedCategories && !isCancelled) {
                categories.current = fetchedCategories;
                if (categories.current.length > 0 && props.setDefaultCategory) {
                    setSelectedCategoryId.current(fetchedCategories[0].id!);
                }
            }
        };
        fetchCategories();
        return () => {
            isCancelled = true;
        }
    }, [props.setDefaultCategory]);

    const handleClick = (id: string) => {
        props.setSelectedCategoryId(id);
        if (props.setPage) {
            props.setPage(1);
        }
    }

    return(
        <div>
            {categories.current?.map(category => {
                return (
                    <button key={category.id} 
                        className={category.id === props.selectedCategoryId ? "purple" : ""}
                        onClick={() => {handleClick(category.id!)}} >
                        {category.category}
                    </button>
                );
            })}
        </div>
    );
}

export default ReviewCategories;