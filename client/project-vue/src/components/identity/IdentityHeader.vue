<script setup lang="ts">
import { useJwtStore } from '@/stores/jwt';
import { getUserName } from '@/helpers/IdentityHelpers';
import { IdentityService } from '@/services/IdentityService';
import router from '@/router';

const jwtStore = useJwtStore();

const identityService = new IdentityService();

const logout = (e: any) => {
    e.preventDefault();
    if (jwtStore.jwtResponse)
        identityService.logout(jwtStore.jwtResponse).then((response) => {
            if (response) {
                jwtStore.jwtResponse = null;
                router.push("/");
            }
    });
}

</script>

<template>
    <template v-if="jwtStore.jwtResponse">
        <li class="nav-item">
            <span class="nav-link text-dark">
                {{ getUserName(jwtStore.jwtResponse) ?? "" }}
            </span>
        </li>
        <li class="nav-item">
            <button @click="(e) => logout(e)" class="fake-button nav-link text-dark">Logout</button>
        </li>
    </template>

    <template v-else>
        <li class="nav-item">
            <RouterLink :to="'/register'" class="nav-link text-dark">Register</RouterLink>
        </li>
        <li class="nav-item">
            <RouterLink :to="'/login'" class="nav-link text-dark">Login</RouterLink>
        </li>
    </template>
</template>