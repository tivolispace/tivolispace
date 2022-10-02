import { ApiProperty } from "@nestjs/swagger";

export class HeartbeatDto {
	@ApiProperty({ default: "" })
	hostingInstanceId: string;

	@ApiProperty({ default: false })
	closingGame: boolean;
}
