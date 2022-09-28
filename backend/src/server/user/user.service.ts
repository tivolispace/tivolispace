import { Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { User, UserDocument } from "./user.schema";

@Injectable()
export class UserService {
	constructor(
		@InjectModel(User.name) private userModel: Model<UserDocument>,
	) {}

	async findById(id: string): Promise<User> {
		const user = await this.userModel.findById(id);
		console.log(user);
		return user;
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
}
