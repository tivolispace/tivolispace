import { forwardRef, Module } from "@nestjs/common";
import { MongooseModule } from "@nestjs/mongoose";
import { UserModule } from "../user/user.module";
import { InstanceController } from "./instance.controller";
import { Instance, InstanceSchema } from "./instance.schema";
import { InstanceService } from "./instance.service";

@Module({
	imports: [
		MongooseModule.forFeature([
			{ name: Instance.name, schema: InstanceSchema },
		]),
		forwardRef(() => UserModule),
	],
	providers: [InstanceService],
	controllers: [InstanceController],
	exports: [InstanceService],
})
export class InstanceModule {}
