<script lang="ts">
	import { onMount } from 'svelte';
	import { accessToken, isAdminStore } from '../../stores';
	import { jsPDF } from 'jspdf';

	$: tickets = new Map();

	let accessTokenValue: string;
	let isAdmin: boolean;

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});

	isAdminStore.subscribe((value) => {
		isAdmin = value;
	});

	onMount(() => {
		console.log(accessTokenValue);
		fetch(`http://localhost:5157/api/v1/movies/-1/showings/-1/tickets?PageNumber=1&PageSize=50`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${accessTokenValue}`
			}
		})
			.then(async (response) => {
				if (!response.ok) {
					let err = await response.text();
					console.error(err);
					throw err;
				} else {
					let result = await response.json();
					console.log(accessTokenValue);
					console.log(result);
					let newTickets = new Map(tickets);
					result.forEach(
						(ticket: {
							movieId: any;
							id: number;
							showingNumber: number;
							ticketType: number;
							seat: string;
						}) => {
							if (!newTickets.has(`${ticket.movieId}`)) {
								newTickets.set(`${ticket.movieId}`, []);
							}
							newTickets.get(`${ticket.movieId}`)!.push(ticket);
						}
					);

					for (const [key, value] of newTickets) {
						newTickets.set(key, sortArrayOfObjects(value));
					}
					tickets = newTickets;
					return tickets;
				}
			})
			.catch((error) => console.error('Error getting tickets', error));
	});

	function compareShowings(a: number, b: number) {
		return a <= b;
	}

	function compareSeats(a: string, b: string) {
		let aSplit = a.split('-');
		let bSplit = b.split('-');
		return (
			parseInt(aSplit[0]) * 10 +
			parseInt(aSplit[1]) -
			(parseInt(bSplit[0]) * 10 + parseInt(bSplit[1]))
		);
	}

	function sortArrayOfObjects(array: {
		sort: (
			arg0: (
				a: { showingNumber: number; seat: string },
				b: { showingNumber: number; seat: string }
			) => number | boolean
		) => any;
	}) {
		return array.sort(
			(a: { showingNumber: number; seat: string }, b: { showingNumber: number; seat: string }) => {
				if (a.showingNumber === b.showingNumber) {
					return compareSeats(a.seat, b.seat);
				}
				return compareShowings(a.showingNumber, b.showingNumber);
			}
		);
	}

	async function getMovieName(id: number) {
		const f = await fetch(`http://localhost:5157/api/v1/movies/${id}`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${accessTokenValue}`
			}
		});
		return f.json();
	}

	async function getShowingTime(movieId: number, showing: number) {
		const f = await fetch(`http://localhost:5157/api/v1/movies/${movieId}/showings/${showing}`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				'Authorization': `Bearer ${accessTokenValue}`
			}
		});
		return f.json();
	}

	function dateToUserFriendly(date: string) {
		let splitDate = date.split('T');
		return splitDate[0] + ' ' + splitDate[1].split(':')[0] + ':' + splitDate[1].split(':')[1];
	}

	function ticketTypeToUserFriendly(ticketType: number) {
		switch (ticketType) {
			case 0:
				return 'Basic';
			case 1:
				return 'Premium';
		}
	}

	async function upgrade(
		movieId: number,
		showing: number,
		ticket: { id: any; seat: string; ticketType: number }
	) {
		let data = {
			ticketType: 1,
			seat: ticket.seat
		};
		let newTickets = new Map(tickets);
		let a = newTickets.get(`${movieId}`);
		let matchingTicket = a.find(
			(t: { showingNumber: number; id: any }) => t.showingNumber === showing && t.id === ticket.id
		);
		if (matchingTicket) {
			matchingTicket.ticketType = 1;
		}
		tickets = newTickets;

		const response = await fetch(
			`http://localhost:5157/api/v1/movies/${movieId}/showings/${showing}/tickets/${ticket.id}`,
			{
				method: 'PUT',
				headers: {
					'Content-Type': 'application/json',
					'Authorization': `Bearer ${accessTokenValue}`
				},
				body: JSON.stringify(data)
			}
		);

		if (response.ok) {
			ticket.ticketType = 1;
		} else {
			console.error('Failed to upgrade ticket');
		}
	}

	async function remove(ticket: { movieId: any; showingNumber: any; id: any }) {
		const response = await fetch(
			`http://localhost:5157/api/v1/movies/${ticket.movieId}/showings/${ticket.showingNumber}/tickets/${ticket.id}`,
			{
				method: 'DELETE',
				headers: {
					'Content-Type': 'application/json',
					'Authorization': `Bearer ${accessTokenValue}`
				}
			}
		);

		if (!response.ok) {
			throw await response.json();
		}
		let tIdx = tickets
			.get(`${ticket.movieId}`)
			.findIndex(
				(t: { movieId: any; showingNumber: any; id: any }) =>
					t.movieId === ticket.movieId &&
					t.showingNumber === ticket.showingNumber &&
					t.id === ticket.id
			);
		tickets.get(`${ticket.movieId}`).splice(tIdx, 1);
		tickets = tickets;
		return await response.json();
	}

	async function download(movieId: number, showingNumber: number, id: number) {
		console.log(`${movieId} ${showingNumber} ${id}`);
		const doc = new jsPDF();

		let data = {
			ticketId: '',
			movieId: '',
			showingNumber: '',
			ticketType: 0,
			seat: '',
			userId: '',
			movieTitle: '',
			startTime: '',
			endTime: '',
			price: 0
		};

		await fetch(
			`http://localhost:5157/api/v1/movies/${movieId}/showings/${showingNumber}/tickets/${id}`,
			{
				method: 'GET',
				headers: {
					'Content-Type': 'application/json',
					'Authorization': `Bearer ${accessTokenValue}`
				}
			}
		)
			.then(async (response) => {
				if (!response.ok) {
					throw await response.text();
				} else {
					return await response.json();
				}
			})
			.then(async (r) => {
				data.ticketId = r.id;
				data.movieId = r.movieId;
				data.showingNumber = r.showingNumber;
				data.ticketType = r.ticketType;
				data.seat = r.seat;
				data.userId = r.userId;
				await fetch(`http://localhost:5157/api/v1/movies/${movieId}`, {
					method: 'GET',
					headers: {
						'Content-Type': 'application/json',
						'Authorization': `Bearer ${accessTokenValue}`
					}
				}).then(async (response) => {
					if (!response.ok) {
						throw await response.text();
					} else {
						let m = await response.json();
						data.movieTitle = m.title;
						await fetch(
							`http://localhost:5157/api/v1/movies/${movieId}/showings/${showingNumber}`,
							{
								method: 'GET',
								headers: {
									'Content-Type': 'application/json',
									'Authorization': `Bearer ${accessTokenValue}`
								}
							}
						).then(async (response) => {
							if (!response.ok) {
								throw await response.text();
							} else {
								let s = await response.json();
								data.startTime = s.startTime;
								data.endTime = s.endTime;
								data.price = s.price;
								if (data.ticketType === 1) {
									data.price *= 1.5;
								}
							}
						});
					}
				});
			});

		const image = new Image();
		image.src = `https://api.qrserver.com/v1/create-qr-code/?size=150x150&data=${JSON.stringify(
			data
		)}`;

		await new Promise((resolve) => {
			image.onload = resolve;
		});

		doc.text(`MOVIE THEATRE TICKET`, 10, 10);
		doc.text(`Movie ID: ${data.movieId}`, 10, 20);
		doc.text(`Movie Title: ${data.movieTitle}`, 10, 30);
		doc.text(``, 10, 40);

		doc.text(`Showing Number: ${data.showingNumber}`, 10, 50);
		doc.text(`Start Time: ${dateToUserFriendly(data.startTime)}`, 10, 60);
		doc.text(`End Time: ${dateToUserFriendly(data.endTime)}`, 10, 70);
		doc.text(``, 10, 80);

		doc.text(`Ticket ID: ${data.ticketId}`, 10, 90);
		doc.text(`Ticket Type: ${data.ticketType}`, 10, 100);
		doc.text(`Price: ${data.price}€`, 10, 110);
		doc.text(`Seat: ${data.seat}`, 10, 120);
		doc.text(``, 10, 130);

		doc.text(`User ID: ${data.userId}`, 10, 140);
		doc.addImage(image, 'JPEG', 10, 150, 50, 50);

		// Save the PDF
		doc.save('ticket.pdf');
	}
</script>

<strong class="uppercase text-l"><span style="color:#d4163c">My</span> Tickets:</strong>
{#each [...tickets] as [key, value]}
	{#await getMovieName(key)}
		<p>...waiting</p>
	{:then movie}
		<h2>Movie: {movie.title}</h2>
		<div class="table-container">
			<table class="table table-hover">
				<thead>
					<tr>
						<th>Showing</th>
						<th class="table-sort-asc">Seat</th>
						<th>Price</th>
						<th>Ticket Type</th>
						{#if isAdmin}
							<th>User</th>
						{/if}
					</tr>
				</thead>
				<tbody>
					{#each value as t}
						<tr>
							{#await getShowingTime(key, t.showingNumber)}
								<p>...waiting</p>
							{:then showing}
								<td>
									{dateToUserFriendly(showing.startTime)} -
									{dateToUserFriendly(showing.endTime).split(' ')[1]}
								</td>
								<td>{t.seat}</td>
								<td>{t.ticketType === 1 ? showing.price * 1.5 : showing.price}€</td>
								<td>
									<span class="mr-2">{ticketTypeToUserFriendly(t.ticketType)}</span>
									{#if t.ticketType === 0}
										<button
											type="button"
											on:click={() => upgrade(movie.id, showing.number, t)}
											class="text-blue-500 hover:text-blue-700 mr-2">Upgrade</button
										>
									{/if}

									<button
										type="button"
										on:click={() => remove(t)}
										class="text-red-500 hover:text-red-700 mr-2">Delete</button
									>

									<button
										type="button"
										on:click={() => download(movie.id, showing.number, t.id)}
										class="text-green-500 hover:text-green-500 mr-2">Download</button
									>
								</td>
							{/await}
							{#if isAdmin}
								<td>{t.userId}</td>
							{/if}
						</tr>
					{/each}
				</tbody>
			</table>
		</div>
	{/await}
{/each}
