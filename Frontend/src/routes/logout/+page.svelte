<script>
	import { onMount } from 'svelte';
	import { accessToken, isAdminStore, usernameStore } from '../../stores';
	import { goto } from '$app/navigation';
	import { getToastStore } from '@skeletonlabs/skeleton';

	const toastStore = getToastStore();

	onMount(() => {
		accessToken.set('');
		usernameStore.set('');
		isAdminStore.set(false);
		const t = {
			message: 'Logout successful! ',
			background: 'variant-filled-success'
		};
		toastStore.trigger(t);
		goto('/');
	});
</script>
