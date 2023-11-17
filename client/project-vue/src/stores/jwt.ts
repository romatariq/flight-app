import { ref } from 'vue'
import { defineStore } from 'pinia'
import type { IJWTResponse } from '@/dto/IJwtResponse'

export const useJwtStore = defineStore('jwt', () => {
    const jwtResponse = ref<IJWTResponse | null>(null);

    const setJwtResponse = (data: IJWTResponse | null) => {
        jwtResponse.value = data;
    }

    return { jwtResponse, setJwtResponse }
})
