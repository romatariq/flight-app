<script setup lang="ts">
import type { IReview } from '@/domain/IReview';
import { ReviewReactionService } from '@/services/ReviewReactionService';
import { useJwtStore } from '@/stores/jwt';
import { ref } from 'vue';

const props = defineProps<{
    review: IReview
}>()

const jwtStore = useJwtStore();
const reviewReactionService = new ReviewReactionService();
reviewReactionService.initJwtResponse(jwtStore.jwtResponse, jwtStore.setJwtResponse);

const usersFeedback = ref(props.review.usersFeedback);
const userFeedback = ref(props.review.userFeedback);


const handleSubmit = async (feedback: number) => {
    if (feedback !== 1 && feedback !== -1) return;
    if (userFeedback.value === feedback) feedback = 0;

    let reviewReaction: true | undefined;

    if (feedback === 0) {
        reviewReaction = await reviewReactionService.delete(props.review.id!);
    } else if (userFeedback.value === 0) {
        reviewReaction = await reviewReactionService.add(props.review.id!, feedback);
    } else if (userFeedback.value !== 0) {
        reviewReaction = await reviewReactionService.update(props.review.id!, feedback);
    }

    if (reviewReaction) {
        if (feedback === 0) {
            usersFeedback.value -= userFeedback.value;
        } else {
            usersFeedback.value += (userFeedback.value === 0 ? 1 : 2) * feedback;
        }
        userFeedback.value = feedback;
    }
}

</script>

<template>
    <div>
        <span>{{usersFeedback}}</span>            
        <button v-if="jwtStore.jwtResponse" 
            :class="'user-feedback' + (userFeedback === 1 ? ' selected-positive' : '')" 
            @click="() => {handleSubmit(1)}">+</button>
        <button v-if="jwtStore.jwtResponse" 
            :class="'user-feedback' + (userFeedback === -1 ? ' selected-negative' : '')" 
            @click="() => {handleSubmit(-1)}">-</button>
    </div>
</template>

<style scoped>
button.user-feedback.selected-positive {
    background-color: lightblue;
}
button.user-feedback.selected-negative {
    background-color: lightpink;
}
</style>