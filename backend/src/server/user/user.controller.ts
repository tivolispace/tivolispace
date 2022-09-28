import { Controller, Get, Param, Put, UseGuards } from "@nestjs/common";
import { ApiBearerAuth, ApiTags } from "@nestjs/swagger";
import { TivoliAuthGuard } from "../auth/auth.guard";
import { CurrentUser } from "../auth/user.decorator";
import { UserSessionService } from "./user-session.service";
import { UserService } from "./user.service";

@ApiTags("user")
@Controller("api/user")
export class UserController {
	constructor(
		private readonly userSessionService: UserSessionService,
		private readonly userService: UserService,
	) {}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Put("heartbeat")
	heartbeat(@CurrentUser() user) {
		return this.userSessionService.heartbeatUser(user);
	}

	@Get("profile/:id")
	profile(@Param("id") id: string) {
		return this.userService.getUserProfileById(id);
	}
}
