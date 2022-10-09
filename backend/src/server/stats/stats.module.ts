import { Module } from "@nestjs/common";
import { InstanceModule } from "../instance/instance.module";
import { UserModule } from "../user/user.module";
import { StatsController } from "./stats.controller";

@Module({
	imports: [UserModule, InstanceModule],
	providers: [],
	controllers: [StatsController],
})
export class StatsModule {}
