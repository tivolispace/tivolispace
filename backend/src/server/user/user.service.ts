import { Injectable, NotFoundException } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { User, UserDocument } from "./user.schema";
import axios from "axios";
import { STEAM_WEB_API_KEY } from "../environment";

@Injectable()
export class UserService {
	constructor(
		@InjectModel(User.name) private readonly userModel: Model<UserDocument>,
	) {}

	async findById(id: string): Promise<User> {
		return this.userModel.findById(id);
	}

	async findBySteamId(steamId: string): Promise<User> {
		return this.userModel.findOne({ steamId });
	}

	async createUser(steamId: string) {
		const user = new this.userModel({
			steamId,
		});
		await user.save();
		return user;
	}

	// TODO: need to cache this because we cant be requesting too much data from steam all the time

	async getProfileById(userId: string) {
		const user = await this.findById(userId);
		if (user == null) throw new NotFoundException("User ID not found");

		const profileRes = await axios(
			"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002",
			{
				params: {
					key: STEAM_WEB_API_KEY,
					steamids: user.steamId,
				},
			},
		);

		const profile: {
			response: {
				players: {
					steamid: string;
					communityvisibilitystate: number;
					profilestate: number;
					personaname: string;
					commentpermission: number;
					profileurl: string;
					avatar: string;
					avatarmedium: string;
					avatarfull: string;
					avatarhash: string;
					personastate: number;
					primaryclanid: string;
					timecreated: number;
					personastateflags: number;
					loccountrycode: "US";
				}[];
			};
		} = profileRes.data;

		if (profile.response.players.length == 0) {
			throw new NotFoundException("Steam profile not found");
		}

		const player = profile.response.players[0];

		return {
			id: userId,
			name: player.personaname,
			avatar: player.avatarfull,
		};
	}
}
