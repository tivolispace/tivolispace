import { Module } from "@nestjs/common";
import { InstanceService } from "./instance.service";
import { InstanceController } from "./instance.controller";
import { UserModule } from "../user/user.module";

@Module({
	imports: [UserModule],
	providers: [InstanceService],
	controllers: [InstanceController],
})
export class InstanceModule {}
