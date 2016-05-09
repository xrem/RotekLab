using System;

namespace Parser
{

    public enum FpcAction
    {
        fpcActionNotLogged = 0,
        fpcActionBind = 1,
        fpcActionListen = 2,
        fpcActionGHBN = 3,
        fpcActionGHBA = 4,
        fpcActionRedirectBind = 5,
        fpcActionEstablish = 6,
        fpcActionTerminate = 7,
        fpcActionDenied = 8,
        fpcActionAllowed = 9,
        fpcActionFailed = 10,
        fpcActionIntermediate = 11,
        fpcActionSuccessfulConnection = 12,
        fpcActionUnsuccessfulConnection = 13,
        fpcActionDisconnection = 14,
        fpcActionUserclearedQuarantine = 15,
        fpcActionQuarantinetimeout = 16
    };

    public static class FpcActionHelper
    {
        public static string ActionToString(FpcAction x)
        {
            switch (x)
            {
                case FpcAction.fpcActionNotLogged:
                    return "No action was logged.";
                case FpcAction.fpcActionBind:
                    return "The Firewall service associated a local address with a socket.";
                case FpcAction.fpcActionListen:
                    return "The Firewall service placed a socket in a state in which it listens for an incoming connection.";
                case FpcAction.fpcActionGHBN:
                    return "The Firewall service retrieved host information corresponding to a host name.";
                case FpcAction.fpcActionGHBA:
                    return "The Firewall service retrieved host information corresponding to a network address.";
                case FpcAction.fpcActionRedirectBind:
                    return "The Firewall service enabled a connection using a local address associated with a socket.";
                case FpcAction.fpcActionEstablish:
                    return "The Firewall service established a session.";
                case FpcAction.fpcActionTerminate:
                    return "The Firewall service terminated a session.";
                case FpcAction.fpcActionDenied:
                    return "The Firewall service denied a request.";
                case FpcAction.fpcActionAllowed:
                    return "The Firewall service allowed a request.";
                case FpcAction.fpcActionFailed:
                    return "The action requested failed.";
                case FpcAction.fpcActionIntermediate:
                    return "The Firewall service created an intermediate log entry containing the connection properties and the amount of traffic passed for a connection that has existed for more than 15 min.";
                case FpcAction.fpcActionSuccessfulConnection:
                    return "The Firewall service was successful in establishing a connection to a socket.";
                case FpcAction.fpcActionUnsuccessfulConnection:
                    return "The Firewall service was unsuccessful in establishing a connection to a socket.";
                case FpcAction.fpcActionDisconnection:
                    return "The Firewall service closed a connection on a socket.";
                case FpcAction.fpcActionUserclearedQuarantine:
                    return "The Firewall service cleared a quarantined VPN client.";
                case FpcAction.fpcActionQuarantinetimeout:
                    return "The Firewall service disqualified a quarantined VPN client after the time-out period elapsed.";
            }
            return "Invalid FpcAction";
        }
    }
}