import { forwardRef, Inject, Injectable } from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { InstanceService } from "../instance/instance.service";
import { HeartbeatDto } from "./heartbeat.dto";
import {
	HeartbeatTimeMs,
	UserSession,
	UserSessionDocument,
} from "./user-session.schema";
import { User } from "./user.schema";

@Injectable()
export class UserSessionService {
	constructor(
		@InjectModel(UserSession.name)
		private readonly userSessionModel: Model<UserSessionDocument>,
		@Inject(forwardRef(() => InstanceService))
		private readonly instanceService: InstanceService,
	) {}

	async heartbeatUser(user: User, heartbeatDto: HeartbeatDto) {
		if (heartbeatDto.closingGame) {
			await this.userSessionModel.deleteOne({ user }).catch(() => {});
			return { id: null };
		}

		let session = await this.userSessionModel.findOne({ user });
		if (session == null) {
			session = new this.userSessionModel({ user });
		}

		session.expiresAt = new Date(Date.now() + HeartbeatTimeMs);

		await session.save();

		if (
			heartbeatDto.hostingInstanceId != null &&
			heartbeatDto.hostingInstanceId != ""
		) {
			this.instanceService
				.heartbeatInstance(user, heartbeatDto.hostingInstanceId)
				.catch(() => {});
		}

		return { id: session.id };
	}

	async isOnline(user: User) {
		const session = await this.userSessionModel.findOne({ user });
		return session != null && session.expiresAt > new Date();
	}

	async onlineUserIds() {
		const sessions = await this.userSessionModel.find({
			expiresAt: { $gte: Date.now() },
		});
		return sessions.map(session => session.user as any as string);
	}
}
