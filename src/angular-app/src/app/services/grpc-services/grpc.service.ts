import { inject, Injectable } from '@angular/core';
import {
  Client,
  ClientMiddlewareCall,
  CompatServiceDefinition,
  createChannel,
  createClientFactory,
  Metadata
} from 'nice-grpc-web';
import {CallOptions, ClientMiddleware} from 'nice-grpc-common';
import {environment} from '../../../environments/environment';
import { MetadataService } from '../metadata.service';
@Injectable({
  providedIn: 'root',
})
export class GrpcService {
  channel = createChannel(environment.baseGrpcUrl);
  metadata = inject(MetadataService);
  clientFactory = createClientFactory().use(this.authMiddleware.bind(this));

  public getClient<Service extends CompatServiceDefinition>(definition: Service, middleware?: ClientMiddleware): Client<Service>{
    if(middleware)
      return this.clientFactory.use(middleware).create(definition, this.channel);
    return this.clientFactory.create(definition, this.channel)
  }
  async *authMiddleware<Request, Response>(
    call: ClientMiddlewareCall<Request, Response>,
    options: CallOptions,
  ){
    const playerMetadata = this.metadata.getPlayerId()
    return yield* call.next(call.request, {
      ...options,
      metadata: Metadata(options.metadata).set('x-player-id', playerMetadata ?? '')
    });
  }
}
export type Middleware = AsyncGenerator<Awaited<Response>, void | Awaited<Response>, undefined>;