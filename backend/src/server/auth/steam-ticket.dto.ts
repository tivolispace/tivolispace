import { ApiProperty } from "@nestjs/swagger";
import { IsNotEmpty } from "class-validator";

export class SteamTicketDto {
	@IsNotEmpty()
	@ApiProperty({ default: "hex string" })
	ticket: string;
}
