import * as WindowsAzure from 'azure-mobile-apps-client';

export default class ShellmongerApp {
    constructor() {
    this.client = new WindowsAzure.MobileServiceClient(this.clientUrl);
    }
}
