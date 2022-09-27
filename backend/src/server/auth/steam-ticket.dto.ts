import { ApiProperty } from "@nestjs/swagger";

export class SteamTicketDto {
	@ApiProperty({ default: "hex string" })
	ticket: string;
}
