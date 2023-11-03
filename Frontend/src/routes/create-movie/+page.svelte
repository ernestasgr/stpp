<script lang="ts">
	import { accessToken } from '../../stores';

	$: movie = {
		title: '',
		description: '',
		releaseDate: '',
		director: '',
		mainImage: '',
		images: [''] as string[],
		videos: [''] as string[]
	};

	let showingTimes: any[] = [];

	let createResult: string;
	let createSuccessful = false;

	let accessTokenValue: string;

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});

	function addImage() {
		movie.images = [...movie.images, ''];
	}

	/**
	 * @param {number} index
	 */
	function removeImage(index: number) {
		movie.images = movie.images.filter((_, i) => i !== index);
	}

	function addVideo() {
		movie.videos = [...movie.videos, ''];
	}

	/**
	 * @param {number} index
	 */
	function removeVideo(index: number) {
		movie.videos = movie.videos.filter((_, i) => i !== index);
	}

	function sleep(ms: number | undefined) {
		return new Promise((resolve) => setTimeout(resolve, ms));
	}

	function createMovie() {
		let data = {
			title: movie.title,
			description: movie.description,
			releaseDate: movie.releaseDate + 'T12:00:00Z',
			director: movie.director,
			mainImage: movie.mainImage,
			images: movie.images,
			videos: movie.videos
		};

		fetch('http://localhost:5157/api/v1/movies', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${accessTokenValue}`
			},
			body: JSON.stringify(data)
		})
			.then(async (response) => {
				console.log(response);
				if (!response.ok) {
					createResult = await response.text();
					console.log(createResult);
					createSuccessful = false;
					throw createResult;
				} else {
					return response.json();
				}
			})
			.then((response) => {
				createResult = 'Movie created successfully!';
				createSuccessful = true;
				console.log(response);
				return response;
			})
			.then(async (response) => {
				console.log(response);
				for (let i = 0; i < showingTimes.length; i++) {
					let showingToUtc = {
						startTime: showingTimes[i].startTime + ':00Z',
						endTime: showingTimes[i].endTime + ':00Z'
					};
					console.log(showingToUtc);
					await fetch(`http://localhost:5157/api/v1/movies/${response.id}/showings`, {
						method: 'POST',
						headers: {
							'Content-Type': 'application/json',
							Authorization: `Bearer ${accessTokenValue}`
						},
						body: JSON.stringify(showingToUtc)
					})
						.then(async (response) => {
							console.log(response);
							if (!response.ok) {
								createResult = await response.text();
								console.log(createResult);
								createSuccessful = false;
								throw createResult;
							} else {
								return response.json();
							}
						})
						.catch((error) => console.error('Error creating showing', error));
				}
			})
			.then((response) => {
				createResult = 'Movie created successfully!';
				createSuccessful = true;
				console.log(response);
			})
			.catch((error) => console.error('Error creating movie', error));
	}

	function removeShowingTime(index: number): any {
		showingTimes = showingTimes.filter((_, i) => i !== index);
	}

	function addShowingTime() {
		showingTimes = [...showingTimes, { startTime: '', endTime: '' }];
		console.log(showingTimes);
	}
</script>

<main class="p-4">
	<h1 class="text-3xl font-semibold mb-4">Create a Movie</h1>

	<form on:submit|preventDefault={createMovie}>
		<div class="mb-4">
			<label for="title" class="block text-sm font-medium mb-2">Title</label>
			<input type="text" id="title" bind:value={movie.title} class="input" />
		</div>

		<div class="mb-4">
			<label for="description" class="block text-sm font-medium mb-2">Description</label>
			<textarea id="description" bind:value={movie.description} class="input" rows="4" />
		</div>

		<div class="mb-4">
			<label for="releaseDate" class="block text-sm font-medium mb-2">Release Date</label>
			<input type="date" id="releaseDate" bind:value={movie.releaseDate} class="input" />
		</div>

		<div class="mb-4">
			<label for="director" class="block text-sm font-medium mb-2">Director</label>
			<input type="text" id="director" bind:value={movie.director} class="input" />
		</div>

		<div class="mb-4">
			<label for="mainImage" class="block text-sm font-medium mb-2">Main Image</label>
			<input type="url" bind:value={movie.mainImage} class="input" />
		</div>

		<div class="mb-4">
			<label for="images" class="block text-sm font-medium mb-2">Images</label>
			{#each movie.images as image, index}
				<div class="flex items-center mb-2">
					<input type="url" bind:value={movie.images[index]} class="input mr-2" />
					<button
						type="button"
						on:click={() => removeImage(index)}
						class="text-red-500 hover:text-red-700">Remove</button
					>
				</div>
			{/each}
			<button type="button" on:click={addImage} class="btn btn-sm variant-filled">Add Image</button>
		</div>

		<div class="mb-4">
			<label for="videos" class="block text-sm font-medium mb-2">Videos</label>
			{#each movie.videos as video, index}
				<div class="flex items-center mb-2">
					<input type="url" bind:value={movie.videos[index]} class="input mr-2" />
					<button
						type="button"
						on:click={() => removeVideo(index)}
						class="text-red-500 hover:text-red-700">Remove</button
					>
				</div>
			{/each}
			<button type="button" on:click={addVideo} class="btn btn-sm variant-filled">Add Video</button>
		</div>

		<div class="flex">
			<div class="mb-4">
				<label for="showingTimes" class="block text-sm font-medium mb-2">Showing Time</label>
				{#each showingTimes as showing, index}
					<div class="flex items-center mb-2">
						<div class="mr-4">
							<label for="startTime" class="block text-xs font-medium mb-1">Start Time</label>
							<input
								type="datetime-local"
								bind:value={showingTimes[index].startTime}
								class="input mb-2"
								placeholder="Start Time"
							/>
						</div>
						<div class="mr-4">
							<label for="endTime" class="block text-xs font-medium mb-1">End Time</label>
							<input
								type="datetime-local"
								bind:value={showingTimes[index].endTime}
								class="input mb-2"
								placeholder="End Time"
							/>
						</div>
						<button
							type="button"
							on:click={() => removeShowingTime(index)}
							class="text-red-500 hover:text-red-700">Remove</button
						>
					</div>
				{/each}
				<button type="button" on:click={addShowingTime} class="btn btn-sm variant-filled"
					>Add Showing</button
				>
			</div>
		</div>

		<button type="submit" class="btn variant-filled">Create Movie</button>
	</form>
</main>

{#if createResult}
	<p>{createResult}</p>
{/if}