import { Controller, Get } from "@nestjs/common";
import { ApiTags } from "@nestjs/swagger";
import { InstanceService } from "../instance/instance.service";
import { UserSessionService } from "../user/user-session.service";

@ApiTags("stats")
@Controller("api/stats")
export class StatsController {
	constructor(
		private readonly userSessionService: UserSessionService,
		private readonly instanceService: InstanceService,
	) {}

	@Get("online")
	async online() {
		return {
			users: this.userSessionService.onlineCount(),
			instances: this.instanceService.onlineCount(),
		};
	}

	// TODO: replace with friends
	@Get("online-users")
	async onlineUsers() {
		// TODO: terrible but its ok for now

		// const count = await this.userSessionService.onlineCount();
		const userIds = await this.userSessionService.onlineUserIds();

		return {
			count: userIds.length,
			userIds,
		};
	}
}
