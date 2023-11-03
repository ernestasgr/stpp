<script lang="ts">
	import { onMount } from 'svelte';
	import { accessToken } from '../../stores';

	$: tickets = new Map();

	let accessTokenValue: string;

	accessToken.subscribe((value) => {
		accessTokenValue = value;
	});

	onMount(() => {
		fetch(`http://localhost:5157/api/v1/movies/-1/showings/-1/tickets?PageNumber=1&PageSize=50`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${accessTokenValue}`
			}
		})
			.then(async (response) => {
				if (!response.ok) {
					throw await response.text();
				} else {
					let result = await response.json();
					console.log(accessTokenValue);
					console.log(result);
					let newTickets = new Map(tickets);
					result.forEach(
						(ticket: {
							movieId: any;
							id: number;
							price: number;
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
					console.log(newTickets);
					tickets = newTickets;
					console.log(tickets);
					return tickets;
				}
			})
			.catch((error) => console.error('Error getting tickets', error));
	});

	async function getMovieName(id: number) {
		const f = await fetch(`http://localhost:5157/api/v1/movies/${id}`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${accessTokenValue}`
			}
		});
		return f.json();
	}

	async function getShowingTime(movieId: number, showing: number) {
		const f = await fetch(`http://localhost:5157/api/v1/movies/${movieId}/showings/${showing}`, {
			method: 'GET',
			headers: {
				'Content-Type': 'application/json',
				Authorization: `Bearer ${accessTokenValue}`
			}
		});
		return f.json();
	}

	function dateToUserFriendly(date: string) {
		let splitDate = date.split('T');
		return splitDate[0] + ' ' + splitDate[1].split(':')[0] + ':' + splitDate[1].split(':')[1];
	}
</script>

<strong class="uppercase text-l"><span style="color:#d4163c">My</span> Tickets:</strong>
{#each [...tickets] as [key, value]}
	{#await getMovieName(key)}
		<p>...waiting</p>
	{:then movie}
		<h2>Movie: {movie.title}</h2>
	{/await}
	<div class="table-container">
		<table class="table table-hover">
			<thead>
				<tr>
					<th>Showing</th>
					<th>Seat</th>
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
						{/await}
						<td>{t.seat}</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</div>
{/each}
