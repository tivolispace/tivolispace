import { Body, Controller, Get, Param, Post, UseGuards } from "@nestjs/common";
import { ApiBearerAuth, ApiTags } from "@nestjs/swagger";
import { TivoliAuthGuard } from "../auth/auth.guard";
import { CurrentUser } from "../auth/user.decorator";
import { User } from "../user/user.schema";
import { CreateInstanceDto } from "./create-instance.dto";
import { InstanceService } from "./instance.service";

@ApiTags("instance")
@Controller("api/instance")
export class InstanceController {
	constructor(private readonly instanceService: InstanceService) {}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Get("all")
	async getAllInstances(@CurrentUser() user) {
		const instances = await this.instanceService.getInstances();
		return { instances };
	}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Post("start")
	startInstance(
		@CurrentUser() user: User,
		@Body() createInstanceDto: CreateInstanceDto,
	) {
		return this.instanceService.startInstance(user, createInstanceDto);
	}

	@ApiBearerAuth()
	@UseGuards(TivoliAuthGuard)
	@Post("close/:id")
	closeInstance(@CurrentUser() user: User, @Param() instanceId) {
		return this.instanceService.closeInstance(user, instanceId);
	}
}
