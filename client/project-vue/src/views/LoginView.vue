<script setup lang="ts">
import type { ILoginData } from '@/dto/ILoginData';
import router from '@/router';
import { IdentityService } from '@/services/IdentityService';
import { useJwtStore } from '@/stores/jwt';
import { ref } from 'vue';
import ValidationErrors from '@/components/ValidationErrors.vue';

const identityService = new IdentityService();
const jwtStore = useJwtStore()

const values = ref({
    email: "",
    password: "",
} as ILoginData);

const validationErrors = ref([] as string[]);



const handleClick = async (e: MouseEvent) => {
    e.preventDefault();

    if (values.value.email.length === 0 || values.value.password.length === 0) {
        validationErrors.value = ["Bad input values!"];
        return;
    }

    // remove errors
    validationErrors.value = [];
    var jwtData = await identityService.login(values.value);

    if (jwtData === undefined) {
        validationErrors.value = ["wrong email or password"];
        return;
    }
    if (jwtData === null) {
        validationErrors.value = ["You have not been verified yet! Try again later!"];
        return;
    }
    jwtStore.jwtResponse = jwtData;
    router.push("/");
}
</script>

<template>
    <form class="form-signin w-100 m-auto">
        <h2>Login</h2>
        <hr />

        <ValidationErrors :validation-errors="validationErrors"   />

        <div class="form-floating mb-3">
            <input v-model="values.email" className="form-control" autoComplete="username" aria-required="true"
                placeholder="name@example.com" type="email" id="Input_Email" name="email" />
            <label htmlFor="Input_Email">Email</label>
        </div>
        <div class="form-floating mb-3">
            <input v-model="values.password" className="form-control" autoComplete="new-password" aria-required="true"
                placeholder="password" type="password" id="Input_Password" name="password" />
            <label htmlFor="Input_Password">Password</label>
        </div>

        <button @click="(e) => handleClick(e)" id="registerSubmit" className="w-100 btn btn-lg btn-primary">Login</button>

    </form>
</template>