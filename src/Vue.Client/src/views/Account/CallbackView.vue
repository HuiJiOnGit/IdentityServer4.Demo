<template>
  <div>
    <button id="login">Login</button>
  </div>
</template>

<script setup>
import Oidc from "oidc-client";
import { onMounted } from "vue";
import { useRouter } from "vue-router";
const router = useRouter();
// const route = useRoute();
onMounted(() => {
  new Oidc.UserManager({ response_mode: "query" })
    .signinRedirectCallback()
    .then(() => {
      router.push({
        name: "private",
      });
    })
    .catch((e) => {
      console.error(e);
    });
});
</script>
