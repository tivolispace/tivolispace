import { ApiProperty } from "@nestjs/swagger";

export class CreateInstanceDto {
	// @ApiProperty({ required: false, default: "kcp" })
	// transport: string;

	@ApiProperty({ required: true })
	connectionUri: string;
}
