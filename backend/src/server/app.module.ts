import { Module } from "@nestjs/common";
import { ViewModule } from "./view/view.module";
import { ConfigModule, ConfigService } from "@nestjs/config";
import { TypeOrmModule } from "@nestjs/typeorm";

@Module({
	imports: [
		ConfigModule.forRoot({
			isGlobal: true,
		}),
		TypeOrmModule.forRootAsync({
			imports: [ConfigModule],
			inject: [ConfigService],
			useFactory: (configService: ConfigService) => ({
				type: "postgres",
				host: configService.get("dbHost", "localhost"),
				port: configService.get("dbPort", 5432),
				username: configService.get("dbUsername", "postgres"),
				password: configService.get("dbPassword", "postgres"),
				name: configService.get("dbName", "tivolispace"),
				// synchronize: true, // dont use in production
				entities: [],
			}),
		}),
		// import view module last because it uses *
		ViewModule,
	],
	controllers: [],
	providers: [],
})
export class AppModule {}
