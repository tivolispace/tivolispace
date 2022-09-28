import { Controller, Get } from "@nestjs/common";
import { ApiTags } from "@nestjs/swagger";
import { UserSessionService } from "../user/user-session.service";
import { UserService } from "../user/user.service";

@ApiTags("stats")
@Controller("api/stats")
export class StatsController {
	constructor(
		private readonly userSessionService: UserSessionService,
		private readonly userService: UserService,
	) {}

	@Get("online")
	async online() {
		// TODO: terrible code dont do this, but its ok for now

		// const count = await this.userSessionService.onlineCount();
		const userIds = await this.userSessionService.onlineUserIds();

		const users = await Promise.all(
			userIds.map(userId => this.userService.getUserProfileById(userId)),
		);

		return {
			count: userIds.length,
			users,
		};
	}
}
