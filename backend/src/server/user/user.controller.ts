import {
	Controller,
	Get,
	Param,
	Put,
	UseGuards,
	NotFoundException,
} from "@nestjs/common";
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

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Get("profile")
	profile(@CurrentUser() user) {
		return this.userService.getUserProfile(user);
	}

	@Get("profile/:id")
	async profileById(@Param("id") id: string) {
		const user = await this.userService.findById(id);
		if (user == null) throw new NotFoundException("User not found");

		return this.userService.getUserProfile(user);
	}
}
