import { Controller, Get, UseGuards } from "@nestjs/common";
import { ApiBearerAuth, ApiTags } from "@nestjs/swagger";
import { TivoliAuthGuard } from "../auth/auth.guard";
import { CurrentUser } from "../auth/user.decorator";
import { InstanceService } from "./instance.service";

@ApiTags("instance")
@Controller("api/instance")
export class InstanceController {
	constructor(private readonly instanceService: InstanceService) {}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Get("all")
	profile(@CurrentUser() user) {
		return this.instanceService.getInstances();
	}
}
