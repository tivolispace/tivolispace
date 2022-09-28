import { Controller, Get, Put, UseGuards } from "@nestjs/common";
import { ApiBearerAuth, ApiTags } from "@nestjs/swagger";
import { TivoliAuthGuard } from "../auth/auth.guard";
import { CurrentUser } from "../auth/user.decorator";
import { UserSessionService } from "./user-session.service";

@ApiTags("user")
@Controller("api/user")
export class UserController {
	constructor(private readonly userSessionService: UserSessionService) {}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Put("heartbeat")
	heartbeat(@CurrentUser() user) {
		return this.userSessionService.heartbeatUser(user);
	}
}
