import { Module } from "@nestjs/common";
import { StatsController } from "./stats.controller";
import { UserModule } from "../user/user.module";

@Module({
	imports: [UserModule],
	providers: [],
	controllers: [StatsController],
})
export class StatsModule {}
