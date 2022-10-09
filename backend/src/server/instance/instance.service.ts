import {
	BadRequestException,
	Body,
	forwardRef,
	Inject,
	Injectable,
	NotFoundException,
} from "@nestjs/common";
import { InjectModel } from "@nestjs/mongoose";
import { Model } from "mongoose";
import { HeartbeatTimeMs } from "../user/user-session.schema";
import { UserSessionService } from "../user/user-session.service";
import { User } from "../user/user.schema";
import { CreateInstanceDto } from "./create-instance.dto";
import { Instance, InstanceDocument } from "./instance.schema";

@Injectable()
export class InstanceService {
	constructor(
		@InjectModel(Instance.name)
		private readonly instanceModel: Model<InstanceDocument>,
		private readonly userSessionService: UserSessionService,
	) {}

	async heartbeatInstance(owner: User, instanceId: string) {
		const instance = await this.instanceModel.findOne({
			id: instanceId,
			owner,
		});

		instance.expiresAt = new Date(Date.now() + HeartbeatTimeMs);

		await instance.save();

		return { id: instance.id };
	}

	async startInstance(owner: User, createInstanceDto: CreateInstanceDto) {
		if (!(await this.userSessionService.isOnline(owner))) {
			throw new BadRequestException(
				"You need to be online to make an instance",
			);
		}

		const instance = new this.instanceModel({
			owner,
			connectionUri: createInstanceDto.connectionUri,
		});
		await instance.save();
		return { id: instance.id };
	}

	async closeInstance(owner: User, instanceId: string) {
		await this.instanceModel.deleteOne({
			id: instanceId,
			owner,
		});
		return {};
	}

	async getInstances() {
		const instances = await this.instanceModel
			.find({
				expiresAt: { $gte: Date.now() },
			})
			.populate("owner");
		return instances.map(instance => ({
			id: instance.id,
			owner: {
				id: instance.owner.id,
				displayName: instance.owner.displayName,
				profilePictureUrl: instance.owner.profilePictureUrl,
			},
			connectionUri: instance.connectionUri,
		}));
	}

	async onlineCount() {
		return await this.instanceModel.countDocuments({
			expiresAt: { $gte: Date.now() },
		});
	}
}
