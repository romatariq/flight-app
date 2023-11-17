<script setup lang="ts">
const props = defineProps<{
    method: string,
    setSubmitType: (submitType: string) => void
}>()

</script>

<template>
    <button
        @click="() => {props.setSubmitType(props.method.toLowerCase())}" 
        type="submit">{{props.method}}
    </button>
</template>