import { Injectable } from "@nestjs/common";
import { UserSessionService } from "../user/user-session.service";

@Injectable()
export class InstanceService {
	constructor(private readonly userSessionService: UserSessionService) {}

	async getInstances() {
		const hostingUsers = await this.userSessionService.getHostingUsers();
		return {
			instances: hostingUsers.map(user => ({
				name: user.displayName + "'s Instance",
				imageUrl: user.profilePictureUrl,
				steamId: user.steamId,
			})),
		};
	}
}
