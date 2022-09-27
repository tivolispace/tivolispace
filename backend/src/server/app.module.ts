import { Module } from "@nestjs/common";
import { TypeOrmModule } from "@nestjs/typeorm";
import {
	DB_HOST,
	DB_NAME,
	DB_PASSWORD,
	DB_PORT,
	DB_USERNAME,
} from "./environment";
import { ViewModule } from "./view/view.module";
import { AuthModule } from "./auth/auth.module";
import { UserModule } from "./user/user.module";

@Module({
	imports: [
		TypeOrmModule.forRoot({
			type: "postgres",
			host: DB_HOST,
			port: parseInt(DB_PORT),
			username: DB_USERNAME,
			password: DB_PASSWORD,
			name: DB_NAME,
			// synchronize: true, // dont use in production
			entities: [],
		}),
		AuthModule,
		UserModule,
		// import view module last because it uses *
		ViewModule,
	],
	controllers: [],
	providers: [],
})
export class AppModule {}
