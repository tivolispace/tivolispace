import { ApiProperty } from "@nestjs/swagger";

export class HeartbeatDto {
	@ApiProperty({ default: false })
	hosting: boolean;

	@ApiProperty({ default: false })
	closingGame: boolean;
}
