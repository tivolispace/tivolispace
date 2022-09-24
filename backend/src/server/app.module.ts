import { Module } from "@nestjs/common";
import { ViewModule } from "./view/view.module";
import { ConfigModule } from "@nestjs/config";

@Module({
	imports: [
		ConfigModule.forRoot({
			isGlobal: true,
		}),
		ViewModule,
	],
	controllers: [],
	providers: [],
})
export class AppModule {}
