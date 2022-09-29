import { ApiProperty } from "@nestjs/swagger";
import { IsNotEmpty } from "class-validator";

export class HeartbeatDto {
	@IsNotEmpty()
	@ApiProperty({ default: false })
	hosting: boolean;
}
