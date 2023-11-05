<script>
	import { getToastStore } from '@skeletonlabs/skeleton';
	import { accessToken, isAdminStore, usernameStore } from '../../stores';
	import { goto } from '$app/navigation';

	const toastStore = getToastStore();
	let username = '';
	let password = '';
	let loginSuccessful = false;
	let isAdmin = false;
	/**
	 * @type {string}
	 */
	let accessTokenValue;

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});

	$: loginStyle = loginSuccessful ? 'variant-ringed-success' : 'variant-ringed-error';

	/**
	 * @param {any} username
	 * @param {any} password
	 */
	function login(username, password) {
		const data = {
			username: username,
			password: password
		};
		fetch('https://stpp-ernestas-grubis-backend.azurewebsites.net/api/v1/login', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify(data)
		})
			.then(async (response) => {
				if (!response.ok) {
					let l = await response.text();
					loginSuccessful = false;
					throw l;
				} else {
					return response.json();
				}
			})
			.then((response) => {
				accessToken.set(response.accessToken);
				usernameStore.set(username);
				checkAdminRole();
				isAdminStore.set(isAdmin);
				loginSuccessful = true;
				const t = {
					message: 'User logged in successfully!',
					background: 'variant-filled-success'
				};
				toastStore.trigger(t);
			})
			.catch((error) => {
				const t = {
					message: 'Error logging in! ' + error,
					background: 'variant-filled-error'
				};
				toastStore.trigger(t);
			});
		goto('/');
	}

	function checkAdminRole() {
		try {
			const decodedToken = parseJwt(accessTokenValue);
			let roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

			if (decodedToken && roles.includes('Admin')) {
				isAdmin = true;
			} else {
				isAdmin = false;
			}
		} catch (error) {}
	}

	/**
	 * @param {string} token
	 */
	function parseJwt(token) {
		var base64Url = token.split('.')[1];
		var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
		var jsonPayload = decodeURIComponent(
			window
				.atob(base64)
				.split('')
				.map(function (c) {
					return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
				})
				.join('')
		);

		return JSON.parse(jsonPayload);
	}
</script>

<div class="container mx-auto">
	<div class="max-w-md mx-auto">
		<h1 class="text-3xl font-semibold mb-4">Login</h1>
		<form>
			<div class="mb-4">
				<label for="username">Username</label>
				<input
					type="text"
					id="username"
					bind:value={username}
					class="input variant-form-material"
					required
				/>
			</div>

			<div class="mb-4">
				<label for="password">Password</label>
				<input
					type="password"
					id="password"
					bind:value={password}
					class="input variant-form-material"
					required
				/>
			</div>

			<div class="mb-4">
				<button type="button" on:click={() => login(username, password)} class="btn variant-filled">
					Login
				</button>
			</div>
		</form>
	</div>
</div>
