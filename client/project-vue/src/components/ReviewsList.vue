<script setup lang="ts">
import type { IReview } from '@/domain/IReview';
import { ReviewService } from '@/services/ReviewService';
import { useJwtStore } from '@/stores/jwt';
import { ref, watchEffect } from 'vue';
import ReviewStars from './ReviewStars.vue';
import { getFullDateTime, convertUtcToLocal } from '@/helpers/TimeHelpers';
import { getUserId } from '@/helpers/IdentityHelpers';
import ReviewReaction from './ReviewReaction.vue';

const props = defineProps<{
    airportIata: string,
    selectedCategoryId: string,
    setSelectedCategoryId: (id: string) => void
    page: number,
    setPage: (page: number) => void
}>()


const reviewService = new ReviewService();
const jwtStore = useJwtStore();
reviewService.initJwtResponse(jwtStore.jwtResponse, jwtStore.setJwtResponse);

const totalPages = ref(1);
const reviews = ref(undefined as IReview[] | undefined);

const fetchReviews = async () => {
    if (props.selectedCategoryId === "") return;

    if (!props.airportIata || props.airportIata.length !== 3) {
        return;
    }

    const fetchedReviews = await reviewService.getAll(props.airportIata, props.selectedCategoryId, props.page);
    if (fetchedReviews) {
        if (props.page === 1) {
            totalPages.value = fetchedReviews.totalPageCount;
        }
        reviews.value = fetchedReviews.data
            .sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime());
    }
};

watchEffect(() => {
    fetchReviews();
});



</script>

<template>
    <RouterLink :to="'reviews/edit'" v-if="jwtStore.jwtResponse">Add review</RouterLink>
    <button v-if="props.page > 1" 
        class="btn btn-primary" @click="() => props.setPage(props.page-1)">Previous page</button>

    <div class="review-box" v-for="review in reviews" :key="review.id!">
        <ReviewStars :rating="review.rating" />
        <p>{{review.text}}</p>
        <div class="text-right">
            {{review.authorName}} at {{getFullDateTime(convertUtcToLocal(review.createdAt))}}</div>
        <ReviewReaction :review="review" />

        <div v-if="jwtStore.jwtResponse && getUserId(jwtStore.jwtResponse) === review.authorId" 
            class="text-right">
            <RouterLink :to="`reviews/edit/${review.id}`">Edit</RouterLink>
        </div>

    </div>
    <button v-if="props.page < totalPages" 
        class="btn btn-primary" @click="() => props.setPage(props.page - 1)">Next page</button>
</template>