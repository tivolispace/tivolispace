import { Injectable } from "@nestjs/common";

export interface User {
	id: string;
	steamId: string;
}

@Injectable()
export class UserService {
	private users: User[] = [];

	async findById(id: string): Promise<User> {
		return this.users.find(user => user.id == id);
	}

	async findBySteamId(steamId: string): Promise<User> {
		return this.users.find(user => user.steamId == steamId);
	}

	async createUser(steamId: string) {
		const user = {
			id: String(this.users.length),
			steamId,
		};
		this.users.push(user);
		return user;
	}
}
