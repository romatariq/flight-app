<script setup lang="ts">
import { useJwtStore } from '@/stores/jwt';
import ReviewCategories from '@/components/ReviewCategories.vue';
import ReviewStars from '@/components/ReviewStars.vue';
import router from '@/router';
import { ReviewService } from '@/services/ReviewService';
import { getUserId } from '@/helpers/IdentityHelpers';
import type { IReview } from '@/domain/IReview';
import { ref, watchEffect } from 'vue';
import ReviewButton from '@/components/ReviewButton.vue';

const reviewParam = router.currentRoute.value.params['id'];
const reviewId = typeof reviewParam === "string" && reviewParam.length > 0 ? reviewParam : undefined;

const jwtStore = useJwtStore();
const reviewService = new ReviewService();
reviewService.initJwtResponse(jwtStore.jwtResponse, jwtStore.setJwtResponse);

const review = ref(undefined as IReview | undefined);
const rating = ref(0);
const selectedCategoryId = ref("");
const submitType = ref("");
const airportIata = "TLL";

const fetchReview = async () => {
    if (!reviewId) return;

    const fetchedReview = await reviewService.get(reviewId);
    if (fetchedReview) {
        if (fetchedReview.authorId === getUserId(reviewService.jwtResponse)) {
            review.value = fetchedReview;
            rating.value = fetchedReview.rating;
            selectedCategoryId.value = fetchedReview.category.id!;
        }
    }
}

const handleSubmit = async (e: Event) => {
    e.preventDefault();
    if (selectedCategoryId.value === "" || !airportIata) return;

    const reviewForm = e.target as HTMLFormElement;
    const reviewInput = reviewForm["reviewValue"] as HTMLInputElement;

    const review: IReview = {
        id: reviewId,
        airportIata: airportIata,
        text: reviewInput.value,
        rating: rating.value,
        category: {
            id: selectedCategoryId.value,
            category: ""
        },
        authorName: "",
        authorId: getUserId(jwtStore.jwtResponse) ?? "",
        createdAt: new Date(),
        userFeedback: 0,
        usersFeedback: 0
    }

    switch (submitType.value) {
        case "add":
            if (review.rating < 1 || review.rating > 5 || review.text.length < 1 || review.text.length > 1000) return;
            await reviewService.add(review);
            router.push("/reviews");
            return;
        case "save":
            if (review.rating < 1 || review.rating > 5 || review.text.length < 1 || review.text.length > 1000) return;
            await reviewService.update(review);
            router.push("/reviews");
            return;
        case "delete":
            await reviewService.delete(reviewId ?? "");
            router.push("/reviews");
            return;
    }
}

watchEffect(() => {
    fetchReview();
});


</script>

<template>
    <div class="review-page">
        <h2 v-if="!jwtStore.jwtResponse">Sign in</h2>
        <template v-else>
            <h1>{{review ? "Edit" : "Add"}} review</h1>
            <ReviewCategories :setDefaultCategory="reviewId ? false : true" :selectedCategoryId="selectedCategoryId" 
                :setSelectedCategoryId="(id: string) => selectedCategoryId=id"/>
            <form @submit="handleSubmit">
                <br/>
                <ReviewStars :rating="rating" :setRating="(nr: number) => rating=nr"  />
                <br/>
                <textarea name="reviewValue" :defaultValue="review?.text ?? ''" />
                <br/>
                <ReviewButton v-if="!review" method="Add" :setSubmitType="(type: string) => submitType=type" />
                <ReviewButton v-if="review" method="Save" :setSubmitType="(type: string) => submitType=type" />
                <ReviewButton v-if="review" method="Delete" :setSubmitType="(type: string) => submitType=type" />
            </form>
        </template>
    </div>
</template>